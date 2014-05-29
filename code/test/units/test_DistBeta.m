function test_suite = test_DistBeta()
%
initTestSuite;

end

function test_mean()
    d=DistBeta(1, 4);
    assertElementsAlmostEqual(d.mean, 1/5 );
end


function test_variance()
    d=DistBeta(1, 4);
    assertElementsAlmostEqual(d.variance, 2/75);
end

function test_draw()
    d=DistBeta(2,5);
    s=d.draw(50);
    assertTrue(all(s>=0) );
    assertTrue(all(s<=1) );
end

function test_isProper()
    assert(DistBeta(2, 3).isProper());
    assert(~DistBeta(0, 3).isProper());
    assert(~DistBeta(2, 0).isProper());
    assert(~DistBeta(inf, 3).isProper());
    assert(~DistBeta(2, nan).isProper());
    
end

function test_parameters()
    a = 4;
    b = 1;
    d = DistBeta(a, b);
    C = d.parameters;
    assertElementsAlmostEqual(C{1}, a);
    assertElementsAlmostEqual(C{2}, b);
end

function test_distHellinger()
    
    d = DistBeta(5,3);
    
    for i=1:20
        a = rand()*10;
        b = rand()*10;
        d2 = DistBeta(a, b);
        assertElementsAlmostEqual(d2.distHellinger(d2), 0 );
        
        % symmetry
        dist = d.distHellinger(d2);
        assertElementsAlmostEqual(dist, d2.distHellinger(d));
        
        % bound
        assert(dist >= 0);
        assert(dist <=1);
    end
end


